using System;

namespace Server.Items
{
    public class APersonalLetterAddressedToAhie : TransientItem
    {
        [Constructible]
        public APersonalLetterAddressedToAhie() : base(0x14ED, TimeSpan.FromMinutes(30)) => LootType = LootType.Blessed;

        public APersonalLetterAddressedToAhie(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber => 1073128; // A personal letter addressed to: Ahie

        public override bool Nontransferable => true;

        public override void AddNameProperties(IPropertyList list)
        {
            base.AddNameProperties(list);
            AddQuestItemProperty(list);
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();
        }
    }
}