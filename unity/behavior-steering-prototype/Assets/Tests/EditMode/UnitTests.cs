using MonoBehaviours;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    // aka "BT Bot"
    public class BehaviorTestBotBehavior
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

        [Test]
        public void CanBeMovedToDestination()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/BT Bot");
            var gameObject = Object.Instantiate(prefab);
            var sut = gameObject.GetComponent<BTBot>();
            var position = new Vector3(0, 0, 10);
            
            sut.SetDestination(position);

            Assert.IsNull(sut);
        }
    }

    public class PlayerBehavior {}
}