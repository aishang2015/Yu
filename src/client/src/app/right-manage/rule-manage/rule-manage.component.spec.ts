import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RuleManageComponent } from './rule-manage.component';

describe('RuleManageComponent', () => {
  let component: RuleManageComponent;
  let fixture: ComponentFixture<RuleManageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RuleManageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RuleManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
