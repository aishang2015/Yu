import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppRuleComponent } from './app-rule.component';

describe('AppRuleComponent', () => {
  let component: AppRuleComponent;
  let fixture: ComponentFixture<AppRuleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppRuleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppRuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
