namespace EmployeeHub.Common.EmailSender
{
    public static class EmailTemplates
    {
        public const string PasswordResetTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .email-header h1 {{
            color: #007bff;
        }}
        .email-body {{
            margin-bottom: 20px;
        }}
        .email-footer {{
            text-align: center;
            font-size: 12px;
            color: #777;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #007bff;
            text-decoration: none;
            border-radius: 5px;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='email-body'>
            <p>Hi,</p>
            <p>We received a request to reset your password. If you made this request, please click the button below to reset your password:</p>
            <p style='text-align: center;'>
                <a href='{0}' class='button'>Reset Password</a>
            </p>
            <p>If you did not request a password reset, you can safely ignore this email. Your password will remain unchanged.</p>
        </div>
        <div class='email-footer'>
            <p>Thank you,<br>The Book Club Team</p>
        </div>
    </div>
</body>
</html>";

public const string ConfirmEmailTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .email-header h1 {{
            color: #007bff;
        }}
        .email-body {{
            margin-bottom: 20px;
        }}
        .email-footer {{
            text-align: center;
            font-size: 12px;
            color: #777;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: #ffffff;
            background-color: #007bff;
            text-decoration: none;
            border-radius: 5px;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Email Confirmation</h1>
        </div>
        <div class='email-body'>
            <p>Hi,</p>
            <p>Thank you for registering with The Book Club! Please confirm your email address by clicking the button below:</p>
            <p style='text-align: center;'>
                <a href='{0}' class='button'>Confirm Email</a>
            </p>
            <p>If you did not create an account, you can safely ignore this email.</p>
        </div>
        <div class='email-footer'>
            <p>Thank you,<br>The Book Club Team</p>
        </div>
    </div>
</body>
</html>";
    }
}