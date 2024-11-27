﻿using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}