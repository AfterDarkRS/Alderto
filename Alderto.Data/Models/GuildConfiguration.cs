﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alderto.Data.Models
{
    public class GuildConfiguration
    {
        /// <summary>
        /// Default configuration
        /// </summary>
        public static GuildConfiguration DefaultConfiguration => (GuildConfiguration)CfgDefaults.MemberwiseClone();

        /// <summary>
        /// Default value for <see cref="Prefix"/>
        /// </summary>
        public static string DefaultPrefix { get; } = ".";

        /// <summary>
        /// Default value for <see cref="CurrencySymbol"/>
        /// </summary>
        public static string DefaultCurrencySymbol { get; } = "⚽";

        /// <summary>
        /// Default value for <see cref="TimelyRewardQuantity"/>
        /// </summary>
        public static int DefaultTimelyRewardQuantity { get; } = 1;

        /// <summary>
        /// Default value for <see cref="TimelyCooldown"/>
        /// </summary>
        public static int DefaultTimelyCooldown { get; } = 86400; // 24h

        /// <summary>
        /// Default value for <see cref="AcceptedMemberRoleId"/>
        /// </summary>
        public static ulong DefaultAcceptedMemberRoleId { get; } = 0;


        private static readonly GuildConfiguration CfgDefaults = new GuildConfiguration
        {
            Prefix = DefaultPrefix,
            CurrencySymbol = DefaultCurrencySymbol,
            TimelyRewardQuantity = DefaultTimelyRewardQuantity,
            TimelyCooldown = DefaultTimelyCooldown,
            AcceptedMemberRoleId = DefaultAcceptedMemberRoleId
        };


        /// <summary>
        /// Discord guild identifier. Primary and foreign key for <see cref="Models.Guild"/>.
        /// If GuildId > 0, the configuration is determined to be in the database. Make sure you know what you are doing modifying this value.
        /// </summary>
        [Key, ForeignKey(nameof(Guild))]
        public ulong GuildId { get; set; }

        /// <summary>
        /// Prefix for commands.
        /// </summary>
        [MaxLength(20), MinLength(1), Required]
        public string Prefix { get; set; }

        /// <summary>
        /// Text/EmoteString used for displaying currency.
        /// </summary>
        [MaxLength(50), MinLength(1), Required]
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Timely currency claim reward quantity.
        /// </summary>
        public int TimelyRewardQuantity { get; set; }

        /// <summary>
        /// Timely currency claim reward cooldown. This is time measured in seconds.
        /// </summary>
        public int TimelyCooldown { get; set; }

        /// <summary>
        /// Id of role to add the user to, whenever user was accepted to the guild.
        /// </summary>
        public ulong AcceptedMemberRoleId { get; set; }

        /// <summary>
        /// <see cref="Guild"/> of which owns this configuration.
        /// </summary>
        public virtual Guild? Guild { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="GuildConfiguration"/>, with configuration defaults
        /// </summary>
#nullable disable
        private GuildConfiguration() { }
#nullable restore
    }
}
