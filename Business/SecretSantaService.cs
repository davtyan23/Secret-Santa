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
                // Retrieve the group using the token
                var group = await _repository.GetGroupByTokenAsync(invitationToken);
                if (group == null)
                {
                    _loggerAPI.Warn("Invalid or expired invitation token.");
                    throw new InvalidOperationException("Invalid or expired invitation token.");
                }

                int groupId = group.GroupID;

                // Fetch user-group mappings from the UserGroups table
                var userGroups = await _repository.GetUserGroupAsync(groupId);
                if (userGroups == null || userGroups.Count == 0)
                {
                    _loggerAPI.Warn($"No users found in group {groupId}.");
                    throw new InvalidOperationException("No users found in this group.");
                }

                if (userGroups.Count() < 2)
                {
                    _loggerAPI.Warn($"Insufficient users in group {groupId} to perform the draw.");
                    throw new InvalidOperationException("A group must have at least 2 users for the draw.");
                }

                // Extract user IDs from the UserGroups table
                var userIds = userGroups.Select(ug => ug.UserID).ToList();

                // Fetch actual user details using user IDs
                var users = await _repository.GetUsersByIdAsync(userIds);

                var receivers = new List<User>(users);
                var random = new Random();

                // Shuffle receivers list to ensure randomization
                receivers = receivers.OrderBy(_ => random.Next()).ToList();

                // Ensure no user is assigned to themselves
                for (int i = 0; i < users.Count; i++)
                {
                    while (users[i].Id == receivers[i].Id)
                    {
                        receivers = receivers.OrderBy(_ => random.Next()).ToList();
                        i = -1; // Restart check from the beginning
                    }
                }

                // Create the assignments (Santa -> Receiver)
                var assignments = new List<GroupInfo>();
                for (int i = 0; i < users.Count; i++)
                {
                    var santa = users[i];
                    var receiver = receivers[i];

                    assignments.Add(new GroupInfo
                    {
                        UserGroupID = userGroups.First(ug => ug.UserID == santa.Id).UserGroupID,
                        RecieverID = receiver.Id,
                        Whishlist = string.Empty
                    });
                }

                await _repository.SaveGroupInfoAsync(assignments);

                _loggerAPI.Info($"Secret Santa draw completed for group {groupId}.");
            }
            catch (Exception ex)
            {
                _loggerAPI.Error( $"{ex}, An error occurred while performing the Secret Santa draw.");
                throw;
            }
        }


    }
}
