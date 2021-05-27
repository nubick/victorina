using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class SettingsTabsView : ViewBase
    {
        private int _selectedIndex;

        private ViewBase _view1;
        public Image TabBackground1;
        public Text TabText1;

        private ViewBase _view2;
        public Image TabBackground2;
        public Text TabText2;

        public Color DefaultColor;
        public Color SelectedColor;

        public IEnumerator ShowAndWaitForFinish(ViewBase view1, string tabText1, ViewBase view2, string tabText2)
        {
            _selectedIndex = 0;

            _view1 = view1;
            TabText1.text = tabText1;

            _view2 = view2;
            TabText2.text = tabText2;
            
            RefreshUI();
            
            StartCoroutine(WaitForFinishCurrentView());
            
            return ShowAndWaitForFinish();
        }

        private ViewBase CurrentView => _selectedIndex == 0 ? _view1 : _view2;

        private IEnumerator WaitForFinishCurrentView()
        {
            while (CurrentView.IsActive)
                yield return null;

            Hide();
        }

        private void RefreshUI()
        {
            TabBackground1.color = GetColor(0, _selectedIndex);
            TabBackground2.color = GetColor(1, _selectedIndex);
            
            if(_selectedIndex == 0 && _view2.IsActive)
                _view2.Hide();
            
            if(_selectedIndex == 1 && _view1.IsActive)
                _view1.Hide();

            if (_selectedIndex == 0)
                _view1.Show();

            if(_selectedIndex == 1)
                _view2.Show();
        }

        private Color GetColor(int myIndex, int selectedIndex)
        {
            return myIndex == selectedIndex ? SelectedColor : DefaultColor;
        }
        
        public void OnTabClicked(int index)
        {
            _selectedIndex = index;
            RefreshUI();
        }
    }
}