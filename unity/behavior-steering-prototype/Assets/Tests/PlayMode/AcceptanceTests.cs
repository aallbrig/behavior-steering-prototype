using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class AcceptanceTests
    {
        [UnityTest]
        public IEnumerator PlayerCanControlAGameEntity()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Player");
            var sut = Object.Instantiate(prefab);
            yield return null;
            Assert.NotNull(sut);
        }
    }
}