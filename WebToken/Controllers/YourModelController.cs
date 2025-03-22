using Microsoft.EntityFrameworkCore;
using WebToken.Models;
using System.Linq;
using System.Collections.Generic;

namespace WebToken.Services
{
    public class TokenDataService
    {
        private readonly AppDbContext _context;

        public TokenDataService(AppDbContext context)
        {
            _context = context;
        }

        public List<TokenData> GetAllRecords()
        {
            return _context.TokenDatas.ToList();
        }

        public TokenData GetRecordById(int id)
        {
            return _context.TokenDatas.FirstOrDefault(t => t.Id == id);
        }

        public void InsertRecord(TokenData tokenData)
        {
            _context.TokenDatas.Add(tokenData);
            _context.SaveChanges();
        }

        public void UpdateRecord(int id, TokenData updatedTokenData)
        {
            var tokenData = _context.TokenDatas.FirstOrDefault(t => t.Id == id);
            if (tokenData != null)
            {
                tokenData.Token = updatedTokenData.Token;
                tokenData.CreatedAt = updatedTokenData.CreatedAt;
                _context.SaveChanges();
            }
        }

        public void DeleteRecord(int id)
        {
            var tokenData = _context.TokenDatas.FirstOrDefault(t => t.Id == id);
            if (tokenData != null)
            {
                _context.TokenDatas.Remove(tokenData);
                _context.SaveChanges();
            }
        }
    }
}
