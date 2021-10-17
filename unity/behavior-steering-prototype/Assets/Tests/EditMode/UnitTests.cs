using MonoBehaviours;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    // aka "BT Bot"
    public class BehaviorTestBot
    {
        [Test]
        public void CanSetHeadMaterial()
        
        {
            var prefab = Resources.Load<GameObject>("Prefabs/BT Bot");
            var gameObject = Object.Instantiate(prefab);
            var sut = gameObject.GetComponent<BTBot>();
            var testMaterial = new Material(Shader.Find("Specular")); // Note: I don't know what the "specular" shader is

            sut.ChangeHeadMaterial(testMaterial);

            Assert.AreEqual(testMaterial, sut.HeadMaterial);
        }
    }
}