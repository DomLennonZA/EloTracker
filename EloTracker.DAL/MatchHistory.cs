//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EloTracker.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MatchHistory
    {
        public System.Guid ID { get; set; }
        public System.Guid Player1ID { get; set; }
        public System.Guid Player2ID { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public System.Guid WinningPlayerID { get; set; }
    }
}