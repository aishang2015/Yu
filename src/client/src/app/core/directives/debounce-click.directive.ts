import { Directive, OnInit, OnDestroy, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { Subscription, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Directive({
  selector: '[appDebounceClick]'
})
export class DebounceClickDirective implements OnInit, OnDestroy {

  @Input() debounceTime = 2000;

  @Output('appDebounceClick') debounceClick = new EventEmitter();

  private subscription: Subscription;

  private clicks = new Subject<any>();


  ngOnInit(): void {

    // 拦截点击事件然后延迟这些点击事件的执行，直到一段时间内最后一次点击，最后把事件的处理操作交给parent来处理
    this.subscription = this.clicks.pipe(debounceTime(this.debounceTime))
      .subscribe(e => this.debounceClick.emit(e));
  }

  ngOnDestroy(): void {

    // 取消订阅
    this.subscription.unsubscribe();
  }

  // HostListener这个装饰器可以监听directive作用的dom元素的click事件，第二个参数$event告诉Angular传递点击事件到directive中去；
  @HostListener('click', ['$event'])
  clickEvent(event) {
    event.preventDefault();
    event.stopPropagation();
    this.clicks.next(event);
  }

}
