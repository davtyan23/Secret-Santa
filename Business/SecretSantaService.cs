using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class SecretSantaService
    {
        private readonly IRepository _repository;
        private readonly ILoggerAPI _loggerAPI;

        public SecretSantaService(IRepository repository, ILoggerAPI loggerAPI)
        {
            _repository = repository;
            _loggerAPI = loggerAPI;
        }

        public async Task PerformDrawAsync(string invitationToken)
        {
            try
            {
                var group = await _repository.GetGroupByTokenAsync(invitationToken);
                if (group == null)
                {
                    _loggerAPI.Warn("Invalid or expired invitation token.");
                    throw new InvalidOperationException("Invalid or expired invitation token.");
                }

                int groupId = group.GroupID;

                var userGroups = await _repository.GetUserGroupAsync(groupId);
                if (userGroups == null || userGroups.Count < 2)
                {
                    _loggerAPI.Warn($"Insufficient users in group {groupId} to perform the draw.");
                    throw new InvalidOperationException("A group must have at least 2 users for the draw.");
                }

                if (group.IsDrawn == true)
                {
                    throw new InvalidOperationException("The draw has already been performed for this group.");
                }
                // Fetch actual user details
                var users = await _repository.GetUsersByIdAsync(userGroups.Select(ug => ug.UserID).ToList());

                if (users.Count < 2)
                {
                    _loggerAPI.Warn($"Not enough valid users for draw in group {groupId}.");
                    throw new InvalidOperationException("A group must have at least 2 users for the draw.");
                }

                List<User> santa = new List<User>();
                List<User> receiver = new List<User>();

                Random random = new Random();

                // Pairing logic
                for (int i = users.Count; i > 0; i--)
                {
                    var notSantaUser = users.Where(x => !santa.Contains(x)).ToList();
                    var notReceiverUser = users.Where(x => !receiver.Contains(x)).ToList();

                    int randomSantaIndex = random.Next(notSantaUser.Count);
                    int randomReceiverIndex = random.Next(notReceiverUser.Count);

                    while (notSantaUser[randomSantaIndex] == notReceiverUser[randomReceiverIndex])
                    {
                        randomReceiverIndex = random.Next(notReceiverUser.Count);
                    }

                    santa.Add(notSantaUser[randomSantaIndex]);
                    receiver.Add(notReceiverUser[randomReceiverIndex]);
                }

                // Save pairings to database
                List<GroupInfo> assignments = new List<GroupInfo>();

                for (int i = 0; i < users.Count; i++)
                {
                    assignments.Add(new GroupInfo
                    {
                        UserGroupID = userGroups.First(ug => ug.UserID == santa[i].Id).UserGroupID,
                        ReceiverID = receiver[i].Id,
                        Whishlist = string.Empty
                    });
                }
                group.IsDrawn = true;
                await _repository.SaveGroupInfoAsync(assignments);

                _loggerAPI.Info($"Secret Santa draw completed successfully for group {groupId}.");
            }
            catch (Exception ex)
            {
                _loggerAPI.Error($"{ex}, An error occurred while performing the Secret Santa draw.");
                throw;
            }
        }


    }
}
