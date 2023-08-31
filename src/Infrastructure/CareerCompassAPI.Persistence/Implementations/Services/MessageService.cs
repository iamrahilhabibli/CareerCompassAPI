﻿using CareerCompassAPI.Application.Abstraction.Repositories.IMessageRepositories;
using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.Message_DTOs;
using CareerCompassAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CareerCompassAPI.Persistence.Implementations.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageWriteRepository _messageWriteRepository;
        private readonly CareerCompassDbContext _context;

        public MessageService(IMessageWriteRepository messageWriteRepository,
                              CareerCompassDbContext context)
        {
            _messageWriteRepository = messageWriteRepository;
            _context = context;
        }
        public async Task CreateAsync(MessageCreateDto messageCreateDto)
        {
            var senderUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == messageCreateDto.senderId);
            var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == messageCreateDto.receiverId);

            if (senderUser == null || receiverUser == null)
            {
                throw new ArgumentException("Invalid sender or receiver ID");
            }

            var newMessage = new Domain.Entities.Message
            {
                Sender = senderUser,
                Receiver = receiverUser,
                Content = messageCreateDto.content,
                IsRead = false,
                MessageType = messageCreateDto.messageType,
            };
            await _messageWriteRepository.AddAsync(newMessage);
            await _messageWriteRepository.SaveChangesAsync();
        }
    }
}