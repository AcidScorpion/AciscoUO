using System;
using System.Collections.Generic;

namespace Server.Spells.Bushido
{
    public class Confidence : SamuraiSpell
    {
        private static readonly SpellInfo _info = new(
            "Confidence",
            null,
            -1,
            9002
        );

        private static readonly Dictionary<Mobile, TimerExecutionToken> _table = new();
        private static readonly Dictionary<Mobile, TimerExecutionToken> m_RegenTable = new();

        public Confidence(Mobile caster, Item scroll) : base(caster, scroll, _info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(0.25);

        public override double RequiredSkill => 25.0;
        public override int RequiredMana => 10;

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            Caster.FixedEffect(0x37C4, 10, 7, 4, 3);
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.SendLocalizedMessage(1063115); // You exude confidence.

                Caster.FixedParticles(0x375A, 1, 17, 0x7DA, 0x960, 0x3, EffectLayer.Waist);
                Caster.PlaySound(0x51A);

                OnCastSuccessful(Caster);

                BeginConfidence(Caster);
                BeginRegenerating(Caster);
            }

            FinishSequence();
        }

        public static bool IsConfident(Mobile m) => _table.ContainsKey(m);

        public static void BeginConfidence(Mobile m)
        {
            StopConfidenceTimer(m);

            Timer.StartTimer(TimeSpan.FromSeconds(30.0),
                () =>
                {
                    EndConfidence(m);
                    m.SendLocalizedMessage(1063116); // Your confidence wanes.
                },
                out var timerToken
            );

            _table[m] = timerToken;
        }

        private static bool StopConfidenceTimer(Mobile m)
        {
            if (_table.Remove(m, out var timerToken))
            {
                timerToken.Cancel();
                return true;
            }

            return false;
        }

        public static void EndConfidence(Mobile m)
        {
            if (StopConfidenceTimer(m))
            {
                OnEffectEnd(m, typeof(Confidence));
            }
        }

        public static bool IsRegenerating(Mobile m) => m_RegenTable.ContainsKey(m);

        // TODO: Move this to a central regeneration so it actually works properly
        public static void BeginRegenerating(Mobile m)
        {
            StopRegenerating(m);

            // RunUO says this goes for 5 seconds, but UOGuide says 4 seconds during normal regeneration
            var hits = (15 + m.Skills.Bushido.Fixed * m.Skills.Bushido.Fixed / 57600) / 4;

            TimerExecutionToken timerToken = default;
            Timer.StartTimer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), 4,
                () =>
                {
                    // ReSharper disable once AccessToModifiedClosure
                    if (timerToken.RemainingCount == 0)
                    {
                        StopRegenerating(m);
                    }

                    m.Hits += hits;
                },
                out timerToken
            );

            m_RegenTable[m] = timerToken;
        }

        public static void StopRegenerating(Mobile m)
        {
            if (m_RegenTable.Remove(m, out var timer))
            {
                timer.Cancel();
            }
        }
    }
}