using Application.Interfaces;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetCodeAsync(string email, string code)
    {
        var senderEmail = _configuration["EmailSettings:Email"];
        var senderPassword = _configuration["EmailSettings:Password"];

        if(string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
        {
            throw new InvalidOperationException("Falta configurar las credenciales de servicio de email");
        }

        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Soporte TPI", senderEmail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Recuperacion de contraseña";
        message.Body = new TextPart("plain")
        {
            Text = $"""
                Recibimos una solicitud para recuperar tu contraseña.

                Tu código de recuperación es: {code}

                Este código vence en 15 minutos y solo puede utilizarse una vez.
                Si no solicitaste este cambio, podés ignorar este correo.
                """
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        await client.ConnectAsync(
            "smtp.gmail.com",
            587,
            SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(senderEmail, senderPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
