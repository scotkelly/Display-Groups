using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace DisplayGroups
{
    public class Program
    {

        private static void Main(string[] args)
        {
            var startTime = DateTime.Now;

            var membershipGroups = new List<TeamFoundationIdentity>();
            var allIdentities = new Dictionary<IdentityDescriptor, TeamFoundationIdentity>(IdentityDescriptorComparer.Instance);

            if (args.Length == 0)
            {
                ShowArguments();
                return;
            }

            var collectionAddress = args[0];

            var ims = GetIdentityManagementService(collectionAddress);

            AddExpandedMembershipOfValidUsersGroup(ims, membershipGroups, allIdentities);

            // Now output groups and their members. We have to call Read just once more, 
            // to get direct membership of Valid Users group
            var validUsersGroupMembers = GetAllValidUsers(ims);

            DisplayGroupMembers(validUsersGroupMembers, allIdentities);

            foreach (var identity in membershipGroups)
            {
                DisplayGroupMembers(identity, allIdentities);
            }

            Console.WriteLine("======= Finished reading {0} identities in {1} minutes", allIdentities.Count,
                (DateTime.Now - startTime).TotalMinutes);
        }

        private static TeamFoundationIdentity GetAllValidUsers(IIdentityManagementService ims)
        {
            var validUsersGroupMembers = ims.ReadIdentity(GroupWellKnownDescriptors.EveryoneGroup, MembershipQuery.Direct,
                ReadIdentityOptions.None);
            return validUsersGroupMembers;
        }

        private static void AddExpandedMembershipOfValidUsersGroup(IIdentityManagementService ims, List<TeamFoundationIdentity> membershipGroups,
            Dictionary<IdentityDescriptor, TeamFoundationIdentity> allIdentities)
        {
// Get expanded membership of the Valid Users group, which is all identities in this host
            var validUsersGroupMembers = ims.ReadIdentity(GroupWellKnownDescriptors.EveryoneGroup, MembershipQuery.Expanded,
                ReadIdentityOptions.None);

            // If total membership exceeds batch size limit for Read, break it up
            const int batchSizeLimit = 100000;

            if (validUsersGroupMembers.Members.Length > batchSizeLimit)
            {
                BatchUpMembershipThatExceedsSizeLimit(validUsersGroupMembers, batchSizeLimit, ims, membershipGroups,
                    allIdentities);
            }
            else
            {
                var memberIdentities = ims.ReadIdentities(validUsersGroupMembers.Members, MembershipQuery.Direct,
                    ReadIdentityOptions.None);
                AddCollectionUsersAndGroupsToLists(memberIdentities, membershipGroups, allIdentities);
            }
        }

        private static IIdentityManagementService GetIdentityManagementService(string address)
        {
            var tfs =
                TfsTeamProjectCollectionFactory.GetTeamProjectCollection(
                    TfsTeamProjectCollection.GetFullyQualifiedUriForName(address));
            var ims = tfs.GetService<IIdentityManagementService>();
            return ims;
        }

        private static void BatchUpMembershipThatExceedsSizeLimit(TeamFoundationIdentity membershipGroup, int batchSizeLimit,
            IIdentityManagementService ims, List<TeamFoundationIdentity> membershipGroups, Dictionary<IdentityDescriptor, TeamFoundationIdentity> allIdentities)
        {
            var batchNum = 0;
            var remainder = membershipGroup.Members.Length;
            var descriptors = new IdentityDescriptor[batchSizeLimit];
            TeamFoundationIdentity[] memberIdentities;

            while (remainder > 0)
            {
                var startAt = batchNum*batchSizeLimit;
                var length = batchSizeLimit;
                if (length > remainder)
                {
                    length = remainder;
                    descriptors = new IdentityDescriptor[length];
                }

                Array.Copy(membershipGroup.Members, startAt, descriptors, 0, length);
                memberIdentities = ims.ReadIdentities(descriptors, MembershipQuery.Direct, ReadIdentityOptions.None);
                AddCollectionUsersAndGroupsToLists(memberIdentities, membershipGroups, allIdentities);
                remainder -= length;
            }
        }

        private static void ShowArguments()
        {
            Console.WriteLine("DisplayGroups [collection]");
            Console.WriteLine("Example: DisplayGroups.exe https://myaccount.visualstudio.com/DefaultCollection");
        }

        private static void AddCollectionUsersAndGroupsToLists(TeamFoundationIdentity[] identities, List<TeamFoundationIdentity> membershipGroups,
            Dictionary<IdentityDescriptor, TeamFoundationIdentity> allIdentities)
        {
            foreach (var identity in identities)
            {
                allIdentities.Add(identity.Descriptor, identity);

                if (identity.IsContainer)
                {
                    membershipGroups.Add(identity);
                }
            }
        }

        private static void DisplayGroupMembers(TeamFoundationIdentity group, Dictionary<IdentityDescriptor, TeamFoundationIdentity> allIdentities)
        {
            // Output this group's membership
            Console.WriteLine("Members of group: {0}", group.DisplayName);
            Console.WriteLine("=================");

            foreach (var memDesc in group.Members)
            {
                // replace .UniqueName with .DisplayName if you just want the display name. UniqueName shows their email address.
                Console.WriteLine(allIdentities[memDesc].UniqueName);
            }

            Console.WriteLine();
        }
    }
}