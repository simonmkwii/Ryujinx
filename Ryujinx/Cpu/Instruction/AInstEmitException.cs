using ChocolArm64.Decoder;
using ChocolArm64.State;
using ChocolArm64.Translation;
using System.Reflection;

namespace ChocolArm64.Instruction
{
    static partial class AInstEmit
    {
        private const BindingFlags Binding = BindingFlags.NonPublic | BindingFlags.Instance;

        public static void Brk(AILEmitterCtx Context)
        {
            EmitExceptionCall(Context, nameof(ARegisters.OnBreak));
        }

        public static void Svc(AILEmitterCtx Context)
        {
            EmitExceptionCall(Context, nameof(ARegisters.OnSvcCall));
        }

        private static void EmitExceptionCall(AILEmitterCtx Context, string MthdName)
        {
            AOpCodeException Op = (AOpCodeException)Context.CurrOp;

            Context.EmitStoreState();

            Context.EmitLdarg(ATranslatedSub.RegistersArgIdx);

            Context.EmitLdc_I4(Op.Id);

            MethodInfo MthdInfo = typeof(ARegisters).GetMethod(MthdName, Binding);

            Context.EmitCall(MthdInfo);

            if (Context.CurrBlock.Next != null)
            {
                Context.EmitLoadState(Context.CurrBlock.Next);
            }
        }

        public static void Und(AILEmitterCtx Context)
        {
            AOpCode Op = Context.CurrOp;

            Context.EmitStoreState();

            Context.EmitLdarg(ATranslatedSub.RegistersArgIdx);

            Context.EmitLdc_I8(Op.Position);
            Context.EmitLdc_I4(Op.RawOpCode);

            string MthdName = nameof(ARegisters.OnUndefined);

            MethodInfo MthdInfo = typeof(ARegisters).GetMethod(MthdName, Binding);

            Context.EmitCall(MthdInfo);

            if (Context.CurrBlock.Next != null)
            {
                Context.EmitLoadState(Context.CurrBlock.Next);
            }
        }
    }
}