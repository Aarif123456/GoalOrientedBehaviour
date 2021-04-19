namespace GameBrains.AI
{
    using UnityEngine;
    
    public class Steering
    {
        private Types type;
        private Vector3 linear;
        private Vector3 angular;
        
        public enum Types { Velocities, Accelerations, Forces };
        
        public Types Type
        {
            get
            {
                return type;
            }
            
            set
            {
                type = value;
            }
        }
        
        public Vector3 Linear
        {
            get
            {
                return linear;
            }
            
            set
            {
                linear = value;
            }
        }
        
        public Vector3 Angular
        {
            get
            {
                return angular;
            }
            
            set
            {
                angular = value;
            }
        }
        
    }
}