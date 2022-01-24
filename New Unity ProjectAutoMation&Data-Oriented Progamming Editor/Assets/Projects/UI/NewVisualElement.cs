using System.Collections.Generic;
using UnityEngine.UIElements;
namespace UI
{
    public class NewVisualElement : VisualElement
    {

        public string MmMStRing { get; set; }
        void fu()
        {

        }

    #region UxmlFactory

        public new class UxmlFactory : UxmlFactory<NewVisualElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription MyString_A = new() { defaultValue = "Label1", name = "MmMStRing" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield break;
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init( ve, bag, cc );
                var ve_ = ve as NewVisualElement;
                ve_.Clear();
                ve_.MmMStRing = MyString_A.GetValueFromBag( bag, cc );
                ve_.Add( new TextField( "LallaString" ) { value = ve_.MmMStRing } );
            }
        }

    #endregion

    }
}
