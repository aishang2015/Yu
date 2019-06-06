import { Directive, OnInit, OnDestroy, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { Subscription, Subject } from 'rxjs';
import { throttleTime } from 'rxjs/operators'

@Directive({
  selector: '[appThrottleClick]'
})
export class ThrottleClickDirective implements OnInit, OnDestroy {

  @Input() throttleTime = 2000;

  @Output('appThrottleClick') throttleClick = new EventEmitter<any>();

  private subscription: Subscription;

  private clicks = new Subject<any>();

  ngOnInit(): void {

    // 拦截点击事件只传递第一次点击事件的处理操作交给parent来处理
    this.subscription = this.clicks.pipe(throttleTime(this.throttleTime))
      .subscribe(e => this.throttleClick.emit(e));
  }

  ngOnDestroy(): void {

    // 取消订阅
    this.subscription.unsubscribe();
  }

  // HostListener这个装饰器可以监听directive作用的dom元素的click事件，第二个参数$event告诉Angular传递点击事件到directive中去；
  @HostListener("click", ["$event"])
  clickEvent(event: MouseEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.clicks.next(event);
  }

}
