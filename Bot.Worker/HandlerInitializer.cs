using Bot.MessagingFramework;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Bot.Worker
{
    public class HandlerInitializer
    {
        delegate object MethodInvoker();
        MethodInvoker methodHandler = null;

        public HandlerInitializer(Type type)
        {
            CreateMethod(type.GetConstructor(Type.EmptyTypes));
        }

        public HandlerInitializer(ConstructorInfo target)
        {
            CreateMethod(target);
        }

        void CreateMethod(ConstructorInfo target)
        {
            DynamicMethod dynamic = new DynamicMethod(string.Empty,
                        typeof(object),
                        new Type[0],
                        target.DeclaringType);
            ILGenerator il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            methodHandler = (MethodInvoker)dynamic.CreateDelegate(typeof(MethodInvoker));
        }

        public object CreateInstance()
        {
            return methodHandler();
        }

        public static void CreateAllHandlers()
        {
            var type = typeof(IMessageHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var t in types)
            {
                if (t.IsAbstract == false)
                {
                    (new HandlerInitializer(t)).CreateInstance();
                }
            }
        }
    }

    ////Use Above class for Object Creation.
    //ObjectCreateMethod inv = new ObjectCreateMethod(type); //Specify Type
    //Object obj = inv.CreateInstance();
}

