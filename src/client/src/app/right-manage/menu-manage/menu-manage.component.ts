import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-menu-manage',
  templateUrl: './menu-manage.component.html',
  styleUrls: ['./menu-manage.component.scss']
})
export class MenuManageComponent implements OnInit {

  nodes = [
    {
      title: '菜单1', key: '100', icon: 'menu', children: [
        { title: '子菜单1', key: '1001', icon: 'plus-square' },
        { title: '子菜单2', key: '1002', icon: 'link' },
      ]
    }
  ];

  constructor() { }

  ngOnInit() {
  }

  // 创建菜单
  createMenu(tplContent) {
  }

  // 删除菜单
  deleteMenu() {
  }

  // 编辑菜单
  editMenu(tplContent) {
  }


  // 创建菜单
  createElement(tplContent) {
  }

  // 删除菜单
  deleteElement() {
  }

  // 编辑菜单
  editElement(tplContent) {
  }

}
