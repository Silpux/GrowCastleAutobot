namespace gca.Classes.MouseMove
{
    public abstract class MouseMovement
    {

        protected PointF startPoint;
        protected PointF endPoint;

        private float speedFactor = 1f;

        public MouseMovement(PointF startPoint, PointF endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public float SpeedFactor
        {
            get
            {
                return speedFactor;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Speed change factor cannot be 0 or less");
                }
                speedFactor = value;
            }
        }

        protected float t = 0;

        protected abstract PointF GetPoint(float t);
        protected abstract void IncreaseT();

        public IEnumerable<PointF> GetPoints()
        {
            while (t < 1)
            {
                PointF point = GetPoint(t);
                IncreaseT();
                yield return point;
            }
        }

    }
}
