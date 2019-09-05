import { Component, OnInit, Input, Output, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from "@angular/platform-browser";

@Component({
  selector: 'app-node-select',
  templateUrl: './node-select.component.html',
  styleUrls: ['./node-select.component.scss']
})
export class NodeSelectComponent implements OnInit {

  checkedValue = 'startNode';

  nodes = [
    { value: 'startNode', display: '开始节点', html: this.transform('<div style="background:black;border-radius:10px;width:20px;height:20px;"></div>') },
    { value: 'endNode', display: '结束节点', html: this.transform('<div style="background:black;width:20px;height:20px;"></div>') },
    { value: 'workNode', display: '工作节点', html: this.transform('<div style="background:black;width:20px;height:20px;"></div>') }
  ]

  constructor(private sanitizer: DomSanitizer) { }

  ngOnInit() {}

  checked(node) {
    this.checkedValue = node.value;
  }

  transform(style) {
    return this.sanitizer.bypassSecurityTrustHtml(style);
  }
}
