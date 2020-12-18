using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems.Sound
{
    public interface IEventInvoker
    {
         event Action Event;
    }
}
