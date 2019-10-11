import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HandleComponent } from './handle.component';

describe('HandleComponent', () => {
  let component: HandleComponent;
  let fixture: ComponentFixture<HandleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HandleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HandleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
