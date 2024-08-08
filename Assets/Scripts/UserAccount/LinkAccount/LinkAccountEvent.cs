using UnityEngine.Events;

namespace UserAccount.LinkAccount
{
    public class LinkAccountEvent
    {
        public static readonly UnityEvent LinkAccountSuccessful = new UnityEvent();
        
        public static void InvokeLinkAccountSuccessful()
        {
            LinkAccountSuccessful.Invoke();
        }
    }
}