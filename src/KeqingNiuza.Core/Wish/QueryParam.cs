namespace KeqingNiuza.Wish
{
    public struct QueryParam
    {
        /// <summary>
        /// ��Ը����
        /// </summary>
        public WishType WishType { get; set; }

        /// <summary>
        /// ҳ��
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// ��һҳ���һ����id
        /// </summary>
        public long EndId { get; set; }

        public override string ToString()
        {
            return $@"gacha_type={(int)WishType}&page={Page}&size={Size}&end_id={EndId}";
        }
    }
}
