using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace DisplayGroups
{
    internal class Program
    {
        private static readonly Dictionary<IdentityDescriptor, TeamFoundationIdentity> m_identities =
            new Dictionary<IdentityDescriptor, TeamFoundationIdentity>(IdentityDescriptorComparer.Instance);

        private static readonly List<TeamFoundationIdentity> m_groups = new List<TeamFoundationIdentity>();

        private static void Main(string[] args)
        {
            IIdentityManagementService ims;
            TeamFoundationIdentity[] identities;

            if (args.Length == 0)
            {
                ShowArguments();
                return;
            }

            var address = args[0];

            var startTime = DateTime.Now;

            var tfs =
                TfsTeamProjectCollectionFactory.GetTeamProjectCollection(
                    TfsTeamProjectCollection.GetFullyQualifiedUriForName(address));
            ims = tfs.GetService<IIdentityManagementService>();

            // Get expanded membership of the Valid Users group, which is all identities in this host
            var group = ims.ReadIdentity(GroupWellKnownDescriptors.EveryoneGroup, MembershipQuery.Expanded,
                ReadIdentityOptions.None);

            // If total membership exceeds batch size limit for Read, break it up
            var batchSizeLimit = 100000;

            if (group.Members.Length > batchSizeLimit)
            {
                BatchUpMembershipThatExceedsSizeLimit(@group, batchSizeLimit, ims);
            }
            else
            {
                identities = ims.ReadIdentities(group.Members, MembershipQuery.Direct, ReadIdentityOptions.None);
                SortIdentities(identities);
            }

            // Now output groups and their members. We have to call Read just once more, 
            // to get direct membership of Valid Users group
            group = ims.ReadIdentity(GroupWellKnownDescriptors.EveryoneGroup, MembershipQuery.Direct,
                ReadIdentityOptions.None);

            Write(group);

            foreach (var identity in m_groups)
            {
                Write(identity);
            }

            Console.WriteLine("======= Finished reading {0} identities in {1} minutes", m_identities.Count,
                (DateTime.Now - startTime).TotalMinutes);
        }

        private static void BatchUpMembershipThatExceedsSizeLimit(TeamFoundationIdentity @group, int batchSizeLimit,
            IIdentityManagementService ims)
        {
            var batchNum = 0;
            var remainder = @group.Members.Length;
            var descriptors = new IdentityDescriptor[batchSizeLimit];
            TeamFoundationIdentity[] identities;

            while (remainder > 0)
            {
                var startAt = batchNum*batchSizeLimit;
                var length = batchSizeLimit;
                if (length > remainder)
                {
                    length = remainder;
                    descriptors = new IdentityDescriptor[length];
                }

                Array.Copy(@group.Members, startAt, descriptors, 0, length);
                identities = ims.ReadIdentities(descriptors, MembershipQuery.Direct, ReadIdentityOptions.None);
                SortIdentities(identities);
                remainder -= length;
            }
        }

        private static void ShowArguments()
        {
            Console.WriteLine("DisplayGroups [collection]");
            Console.WriteLine("Example: DisplayGroups.exe https://myaccount.visualstudio.com/DefaultCollection");
        }

        private static void SortIdentities(TeamFoundationIdentity[] identities)
        {
            foreach (var identity in identities)
            {
                m_identities.Add(identity.Descriptor, identity);

                if (identity.IsContainer)
                {
                    m_groups.Add(identity);
                }
            }
        }

        private static void Write(TeamFoundationIdentity group)
        {
            // Output this group's membership
            Console.WriteLine("Members of group: {0}", group.DisplayName);
            Console.WriteLine("=================");

            foreach (var memDesc in group.Members)
            {
                // replace .UniqueName with .DisplayName if you just want the display name. UniqueName shows their email address.
                Console.WriteLine(m_identities[memDesc].UniqueName);
            }

            Console.WriteLine();
        }
    }
}