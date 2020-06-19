namespace traVRsal.SDK
{
    public class TileId
    {
        private const uint TiledHexagonal120Flag = 0x10000000;
        private const uint TiledDiagonalFlipFlag = 0x20000000;
        private const uint TiledVerticalFlipFlag = 0x40000000;
        private const uint TiledHorizontalFlipFlag = 0x80000000;

        private enum FlipFlags
        {
            None = 0,
            Diagonal = 1,
            Vertical = 2,
            Horizontal = 4,
            Hexagonal120 = 8
        }

        private uint originalGid;
        private FlipFlags flipFlags;

        public TileId(uint gid)
        {
            originalGid = gid;

            flipFlags = 0;
            flipFlags |= IsFlippedHorizontal() ? FlipFlags.Horizontal : 0;
            flipFlags |= IsFlippedVertical() ? FlipFlags.Vertical : 0;
            flipFlags |= IsFlippedDiagonal() ? FlipFlags.Diagonal : 0;
            flipFlags |= IsFlippedHexagonal120() ? FlipFlags.Hexagonal120 : 0;
        }

        // The tileId with baked in flip flags
        public uint ImportedlTileId { get { return originalGid; } }

        public uint GetRealGid()
        {
            return (uint)(originalGid & ~(TiledHorizontalFlipFlag | TiledVerticalFlipFlag | TiledDiagonalFlipFlag | TiledHexagonal120Flag));
        }

        public bool IsFlippedHorizontal()
        {
            return (originalGid & TiledHorizontalFlipFlag) != 0;
        }

        public bool IsFlippedVertical()
        {
            return (originalGid & TiledVerticalFlipFlag) != 0;
        }

        public bool IsFlippedDiagonal()
        {
            return (originalGid & TiledDiagonalFlipFlag) != 0;
        }

        public bool IsFlippedHexagonal120()
        {
            return (originalGid & TiledHexagonal120Flag) != 0;
        }

        public bool IsFlipped()
        {
            return IsFlippedHorizontal() || IsFlippedVertical() || IsFlippedDiagonal() || IsFlippedHexagonal120();
        }

        public override string ToString()
        {
            return $"TileId {originalGid} ({flipFlags})";
        }
    }
}