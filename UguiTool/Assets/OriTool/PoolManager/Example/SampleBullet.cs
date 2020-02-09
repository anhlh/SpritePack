using OriTool.GoPool;
using UnityEngine;

namespace OriTool.Example
{
    public class SampleBullet : MonoBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletEmitParent;

        // Start is called before the first frame update
        void Start()
        {
            //Init Pool
            GOPoolManager.Init(bulletPrefab, 5);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) EmitBullet();
        }

        private void EmitBullet()
        {
            //get bullet from pool, localPos rotate will be reset by default 
            var bullet = GOPoolManager.Get<Bullet>(bulletEmitParent);
            bullet.Emit();
        }
    }
}