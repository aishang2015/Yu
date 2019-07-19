import { Component, OnInit } from '@angular/core';
import { from, of } from 'rxjs';
import { map, delay, mergeAll, mergeMap, concatAll, concatMap } from 'rxjs/operators';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {

  setting = {
    plugins: 'print preview fullpage searchreplace autolink directionality visualblocks visualchars fullscreen image imagetools link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern help',
    toolbar: 'formatselect | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image media pageembed | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent | removeformat | addcomment',

    language_url: 'assets/tinymce/langs/zh_CN.js',
    language: 'zh_CN',
    skin: 'oxide-dark',

    height: 1000,
    width: 500,

  }

  dataModel = "";

  constructor() { }

  ngOnInit() {
  }

}
