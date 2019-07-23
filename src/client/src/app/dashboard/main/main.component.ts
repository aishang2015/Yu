import { Component, OnInit } from '@angular/core';
import { from, of } from 'rxjs';
import { map, delay, mergeAll, mergeMap, concatAll, concatMap } from 'rxjs/operators';
import { BaseService } from 'src/app/core/services/base.service';
import { DashboardService } from 'src/app/core/services/dashboard.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {

  constructor(private dashboardService: DashboardService) { }

  ngOnInit() {
    this.dashboardService.init();
  }

}
