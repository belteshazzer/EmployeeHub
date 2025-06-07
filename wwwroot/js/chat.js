document.addEventListener('DOMContentLoaded', function() {
    // DOM Elements
    const conversationList = document.getElementById('conversationList');
    const messageContainer = document.getElementById('messageContainer');
    const messageInput = document.getElementById('messageInput');
    const sendButton = document.getElementById('sendButton');
    const chatInput = document.getElementById('chatInput');
    const newChatBtn = document.getElementById('newChatBtn');
    const userSelect = document.getElementById('userSelect');
    const startChatBtn = document.getElementById('startChatBtn');
    const currentChatInfo = document.getElementById('currentChatInfo');
    
    // State
    let currentChatId = null;
    let currentReceiverId = null;
    let currentReceiverName = null;
    
    // SignalR Connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    
    // Start connection
    async function startConnection() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
            loadConversations();
        } catch (err) {
            console.log(err);
            setTimeout(startConnection, 5000);
        }
    }
    
    // SignalR Methods
    connection.on("ReceiveMessage", (message) => {
        if (currentChatId === message.chatId) {
            appendMessage(message, false);
            scrollToBottom();
        }
        updateConversationList(message);
    });
    
    connection.on("MessageUpdated", (chatId, messageId, updatedMessage) => {
        if (currentChatId === chatId) {
            updateMessageInUI(messageId, updatedMessage);
        }
    });
    
    connection.on("MessageDeleted", (chatId, messageId) => {
        if (currentChatId === chatId) {
            deleteMessageInUI(messageId);
        }
    });
    
    connection.on("ChatDeleted", (chatId) => {
        if (currentChatId === chatId) {
            clearChatUI();
        }
        removeConversationFromList(chatId);
    });
    
    // Event Listeners
    sendButton.addEventListener('click', sendMessage);
    messageInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });
    
    newChatBtn.addEventListener('click', showNewChatModal);
    startChatBtn.addEventListener('click', startNewChat);
    
    // Functions
    async function loadConversations() {
        try {
            const response = await fetch('/api/chats');
            const chats = await response.json();
            
            conversationList.innerHTML = '';
            
            if (chats.length === 0) {
                conversationList.innerHTML = '<div class="empty-conversations">No conversations yet</div>';
                return;
            }
            
            chats.forEach(chat => {
                const otherUser = chat.user1Id === currentUserId ? chat.user2 : chat.user1;
                const lastMessage = chat.messages[chat.messages.length - 1];
                
                const conversationItem = document.createElement('div');
                conversationItem.className = 'conversation-item';
                conversationItem.dataset.chatId = chat.id;
                conversationItem.dataset.receiverId = otherUser.id;
                conversationItem.dataset.receiverName = otherUser.name;
                
                conversationItem.innerHTML = `
                    <div class="conversation-avatar">${otherUser.name.charAt(0).toUpperCase()}</div>
                    <div class="conversation-info">
                        <div class="conversation-name">${otherUser.name}</div>
                        <div class="conversation-preview">${lastMessage.content}</div>
                    </div>
                    <div class="conversation-time">${formatTime(lastMessage.timestamp)}</div>
                `;
                
                conversationItem.addEventListener('click', () => {
                    loadChat(chat.id, otherUser.id, otherUser.name);
                });
                
                conversationList.appendChild(conversationItem);
            });
        } catch (error) {
            console.error('Error loading conversations:', error);
        }
    }
    
    async function loadChat(chatId, receiverId, receiverName) {
        currentChatId = chatId;
        currentReceiverId = receiverId;
        currentReceiverName = receiverName;
        
        // Highlight selected conversation
        document.querySelectorAll('.conversation-item').forEach(item => {
            item.classList.remove('active');
            if (item.dataset.chatId === chatId) {
                item.classList.add('active');
            }
        });
        
        // Update header
        currentChatInfo.innerHTML = `
            <div class="d-flex align-items-center">
                <div class="avatar mr-2">${receiverName.charAt(0).toUpperCase()}</div>
                <h4>${receiverName}</h4>
            </div>
        `;
        
        // Show input
        chatInput.style.display = 'block';
        
        try {
            const response = await fetch(`/api/chats/${chatId}/messages`);
            const messages = await response.json();
            
            messageContainer.innerHTML = '';
            
            if (messages.length === 0) {
                messageContainer.innerHTML = '<div class="empty-state"><p>No messages yet</p></div>';
                return;
            }
            
            messages.forEach(message => {
                const isSent = message.senderId === currentUserId;
                appendMessage(message, isSent);
            });
            
            scrollToBottom();
            
            // Join SignalR group for this chat
            await connection.invoke("JoinChat", chatId);
        } catch (error) {
            console.error('Error loading chat:', error);
        }
    }
    
    function appendMessage(message, isSent) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${isSent ? 'message-sent' : 'message-received'}`;
        messageDiv.dataset.messageId = message.id;
        
        const bubbleDiv = document.createElement('div');
        bubbleDiv.className = 'message-bubble';
        bubbleDiv.textContent = message.content;
        
        const infoDiv = document.createElement('div');
        infoDiv.className = 'message-info';
        
        if (message.isEdited) {
            infoDiv.innerHTML = `
                <span>Edited</span>
                <span class="message-time">${formatTime(message.timestamp)}</span>
            `;
        } else {
            infoDiv.innerHTML = `<span class="message-time">${formatTime(message.timestamp)}</span>`;
        }
        
        messageDiv.appendChild(bubbleDiv);
        messageDiv.appendChild(infoDiv);
        
        messageContainer.appendChild(messageDiv);
    }
    
    async function sendMessage() {
        const content = messageInput.value.trim();
        if (!content || !currentChatId || !currentReceiverId) return;
        
        try {
            const response = await fetch('/api/chats/send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    chatId: currentChatId,
                    receiverId: currentReceiverId,
                    content: content
                })
            });
            
            if (response.ok) {
                messageInput.value = '';
            }
        } catch (error) {
            console.error('Error sending message:', error);
        }
    }
    
    function scrollToBottom() {
        messageContainer.scrollTop = messageContainer.scrollHeight;
    }
    
    function formatTime(timestamp) {
        const date = new Date(timestamp);
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }
    
    async function showNewChatModal() {
        try {
            const response = await fetch('/api/users');
            const users = await response.json();
            
            userSelect.innerHTML = '';
            users.forEach(user => {
                if (user.id !== currentUserId) {
                    const option = document.createElement('option');
                    option.value = user.id;
                    option.textContent = user.name;
                    userSelect.appendChild(option);
                }
            });
            
            $('#newChatModal').modal('show');
        } catch (error) {
            console.error('Error loading users:', error);
        }
    }
    
    async function startNewChat() {
        const userId = userSelect.value;
        const userName = userSelect.options[userSelect.selectedIndex].text;
        
        try {
            const response = await fetch('/api/chats/start', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    receiverId: userId
                })
            });
            
            const chat = await response.json();
            
            $('#newChatModal').modal('hide');
            loadChat(chat.id, userId, userName);
            loadConversations();
        } catch (error) {
            console.error('Error starting new chat:', error);
        }
    }
    
    // Initialize
    startConnection();
});