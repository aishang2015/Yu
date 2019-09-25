import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WfformComponent } from './wfform.component';

describe('WfformComponent', () => {
  let component: WfformComponent;
  let fixture: ComponentFixture<WfformComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WfformComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WfformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
