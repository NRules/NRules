using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace NRules.Samples.ClaimsExpert.Domain
{
    public interface IClaimRepository
    {
        Claim GetById(long id);
        IEnumerable<Claim> GetAll();
        void Save(Claim claim);
    }

    internal class ClaimRepository : IClaimRepository
    {
        private readonly Func<ISession> _sessionFactory;

        public ClaimRepository(Func<ISession> sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public Claim GetById(long id)
        {
            using (ISession session = _sessionFactory())
            {
                var claim = session.Query<Claim>().SingleOrDefault(x => x.Id == id);
                if (claim == null)
                {
                    throw new ArgumentException(string.Format("Unknown claim. ClaimId={0}", id));
                }
                return claim;
            }
        }

        public IEnumerable<Claim> GetAll()
        {
            using (ISession session = _sessionFactory())
            {
                var claims = session.Query<Claim>().ToList();
                return claims;
            }
        }

        public void Save(Claim claim)
        {
            using (ISession session = _sessionFactory())
            {
                session.Update(claim);
                session.Flush();
            }
        }
    }
}