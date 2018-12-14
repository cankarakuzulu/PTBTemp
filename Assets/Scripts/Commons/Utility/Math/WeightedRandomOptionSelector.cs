using System;
using System.Text;

namespace nopact.Commons.Utility.Math
{
    public class WeighedRandomOptionSelector
    {
        private float[ ] slotChances, initialCondition;
        private float chanceReductionPerSelectionPercent;
        private ushort optionCount;
        private bool isDebugging;

        public WeighedRandomOptionSelector( ushort optionCount, float chanceReductionPerSelectionPercent, float[] initialCondition = null, bool isDebugging = false )
        {
            this.optionCount = optionCount;
            this.chanceReductionPerSelectionPercent = chanceReductionPerSelectionPercent;
            slotChances = new float[ optionCount ];
            this.isDebugging = isDebugging;
            if ( initialCondition != null )
            {
                this.initialCondition = new float[ initialCondition.Length ];
                Array.Copy( initialCondition, this.initialCondition, initialCondition.Length );
            }                        
            ResetChances();
        }

        public void ResetChances( float[ ] initialConditionOverride = null )
        {
            if ( initialConditionOverride != null )
            {
                Array.Copy( initialConditionOverride, initialCondition, initialConditionOverride.Length );
            }

            if( initialCondition != null && initialCondition.Length == optionCount )
            {
                slotChances = initialCondition;
            }
            else
            {
                slotChances = new float[ optionCount ];
                float chancePerSlot = 1.0f / optionCount;

                for ( int slotIndex = 0; slotIndex < slotChances.Length; slotIndex++ )
                {
                    slotChances[ slotIndex ] = chancePerSlot;
                }
            }

            if ( isDebugging )
            {
                UnityEngine.Debug.Log( string.Format( "{0} - Reset", ToString() ) );
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append( "[WeighedRandomOptionSelector] Slot probabilities :: " );
            for ( int slotIndex = 0; slotIndex < slotChances.Length; slotIndex ++ )
            {
                sb.AppendFormat( "[ {0} -> {1:n3} ]", slotIndex.ToString(), slotChances[ slotIndex ].ToString());
            }
            return sb.ToString();
        }

        private void RecalculateChances( ushort chosenSlotIndex )
        {
            var oldSlotChance = slotChances[ chosenSlotIndex ];
            if ( oldSlotChance - chanceReductionPerSelectionPercent < 0 )
            {
                slotChances[ chosenSlotIndex ] = 0.0f;
            }
            else
            {
                slotChances[ chosenSlotIndex ] -= chanceReductionPerSelectionPercent;
            }

            var deltaSlotChanceReduction = oldSlotChance - slotChances[ chosenSlotIndex ];
            var increasePerSlot = deltaSlotChanceReduction / ( float ) ( slotChances.Length - 1 );

            for ( ushort slotIndex = 0; slotIndex < slotChances.Length; slotIndex++ )
            {
                if ( slotIndex == chosenSlotIndex )
                {
                    continue;
                }
                slotChances[ slotIndex ] += increasePerSlot;
            }

            if ( isDebugging )
            {
                UnityEngine.Debug.Log( string.Format( "{0} - Recalculate", ToString() ) );
            }
        }

        public ushort ChooseASlot()
        {
            float dice =  UnityEngine.Random.Range(0.0f, 1.0f);
            float sum = 0.0f;
            ushort slotIndex = 0;
            for ( slotIndex = 0; slotIndex < slotChances.Length; slotIndex++ )
            {
                sum += slotChances[ slotIndex ];                
                if ( dice < sum )
                {             
                    break;
                }
            }
            if ( isDebugging )
            {
                UnityEngine.Debug.Log( string.Format( "<color=cyan> {2}\n Dice: {0} - Chosen slot: {1} </color>", dice.ToString(), slotIndex.ToString(), ToString() ) );
            }
            RecalculateChances( slotIndex );
            return slotIndex;
        }
    }
}
