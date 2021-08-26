using KeqingNiuza.Model;
using KeqingNiuza.View;
using KeqingNiuza.Wish;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KeqingNiuza.ViewModel
{
    public class WishSummaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WishSummaryViewModel(UserData userData)
        {
            UserData = userData;
            WishDataList = LocalWishLogLoader.Load(userData.WishLogFile);
            WishSummary = WishSummary.Create(userData.WishLogFile);
            CharacterOrder("�Ǽ�");
            WeaponOrder("�Ǽ�");
            if (!IsCorrectOrder)
            {
                WishSummary.CharacterStatistics.Star5List.Reverse();
                WishSummary.CharacterStatistics.Star4List.Reverse();
                WishSummary.WeaponStatistics.Star5List.Reverse();
                WishSummary.WeaponStatistics.Star4List.Reverse();
                WishSummary.PermanentStatistics.Star5List.Reverse();
                WishSummary.PermanentStatistics.Star4List.Reverse();
            }
        }

        public WishSummaryViewModel()
        {
            WishDataList = MainWindowViewModel.WishDataList;
            WishSummary = WishSummary.Create(WishDataList);
            CharacterOrder("�Ǽ�");
            WeaponOrder("�Ǽ�");
            if (!IsCorrectOrder)
            {
                WishSummary.CharacterStatistics.Star5List.Reverse();
                WishSummary.CharacterStatistics.Star4List.Reverse();
                WishSummary.WeaponStatistics.Star5List.Reverse();
                WishSummary.WeaponStatistics.Star4List.Reverse();
                WishSummary.PermanentStatistics.Star5List.Reverse();
                WishSummary.PermanentStatistics.Star4List.Reverse();
            }
        }

        public UserData UserData { get; set; }
        public static List<WishData> WishDataList;

        public bool IsCorrectOrder
        {
            get { return Properties.Settings.Default.IsCorrectOrder; }
            set
            {
                if (IsCorrectOrder != value)
                {
                    if (WishSummary.CharacterStatistics != null)
                    {
                        WishSummary.CharacterStatistics.Star5List = WishSummary.CharacterStatistics.Star5List.Reverse<StarDetail>().ToList();
                        WishSummary.CharacterStatistics.Star4List = WishSummary.CharacterStatistics.Star4List.Reverse<StarDetail>().ToList();
                    }
                    if (WishSummary.WeaponStatistics != null)
                    {
                        WishSummary.WeaponStatistics.Star5List = WishSummary.WeaponStatistics.Star5List.Reverse<StarDetail>().ToList();
                        WishSummary.WeaponStatistics.Star4List = WishSummary.WeaponStatistics.Star4List.Reverse<StarDetail>().ToList();
                    }
                    if (WishSummary.PermanentStatistics != null)
                    {
                        WishSummary.PermanentStatistics.Star5List = WishSummary.PermanentStatistics.Star5List.Reverse<StarDetail>().ToList();
                        WishSummary.PermanentStatistics.Star4List = WishSummary.PermanentStatistics.Star4List.Reverse<StarDetail>().ToList();
                    }
                    Properties.Settings.Default.IsCorrectOrder = value;
                    OnPropertyChanged();
                }
            }
        }


        private WishSummary _WishSummary;
        public WishSummary WishSummary
        {
            get { return _WishSummary; }
            set
            {
                _WishSummary = value;
                OnPropertyChanged();
            }
        }


        private List<ItemInfo> _CharacterInfoList;
        public List<ItemInfo> CharacterInfoList
        {
            get { return _CharacterInfoList; }
            set
            {
                _CharacterInfoList = value;
                OnPropertyChanged();
            }
        }


        private List<ItemInfo> _WeaponInfoList;
        public List<ItemInfo> WeaponInfoList
        {
            get { return _WeaponInfoList; }
            set
            {
                _WeaponInfoList = value;
                OnPropertyChanged();
            }
        }

        private object _DetailContent;
        public object DetailContent
        {
            get { return _DetailContent; }
            set
            {
                _DetailContent = value;
                OnPropertyChanged();
            }
        }




        public void CharacterOrder(string order)
        {
            switch (order)
            {
                case "������":
                    CharacterInfoList = WishSummary.CharacterInfoList.OrderByDescending(x => x.LastGetTime).ThenByDescending(x => x.Count).ToList();
                    break;
                case "����":
                    CharacterInfoList = WishSummary.CharacterInfoList.OrderByDescending(x => x.Count).ThenByDescending(x => x.Rank).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
                case "�Ǽ�":
                    CharacterInfoList = WishSummary.CharacterInfoList.OrderByDescending(x => x.Rank).ThenByDescending(x => x.Count).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
            }
        }

        public void WeaponOrder(string order)
        {
            switch (order)
            {
                case "������":
                    WeaponInfoList = WishSummary.WeaponInfoList.OrderByDescending(x => x.LastGetTime).ThenByDescending(x => x.Count).ToList();
                    break;
                case "����":
                    WeaponInfoList = WishSummary.WeaponInfoList.OrderByDescending(x => x.Count).ThenByDescending(x => x.Rank).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
                case "�Ǽ�":
                    WeaponInfoList = WishSummary.WeaponInfoList.OrderByDescending(x => x.Rank).ThenByDescending(x => x.Count).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
            }
        }


        public void ShowDetailView(string type)
        {
            if (type == "Character")
            {
                var view = new WishItemDetailView(CharacterInfoList);
                view.BackEvent += DetailContent_BackEvent;
                DetailContent = view;
            }
            if (type == "Weapon")
            {
                var view = new WishItemDetailView(WeaponInfoList);
                view.BackEvent += DetailContent_BackEvent;
                DetailContent = view;
            }
        }

        public void ShowDetailView(object dataContext)
        {
            var info = dataContext as ItemInfo;
            if (info.ItemType == "��ɫ")
            {
                var view = new WishItemDetailView(CharacterInfoList, info);
                view.BackEvent += DetailContent_BackEvent;
                DetailContent = view;
            }
            if (info.ItemType == "����")
            {
                var view = new WishItemDetailView(WeaponInfoList, info);
                view.BackEvent += DetailContent_BackEvent;
                DetailContent = view;
            }
        }

        private void DetailContent_BackEvent(object sender, EventArgs e)
        {
            DetailContent = null;
        }
    }
}
