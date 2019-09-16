import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WfdefinitionComponent } from './wfdefinition.component';

describe('WfdefinitionComponent', () => {
  let component: WfdefinitionComponent;
  let fixture: ComponentFixture<WfdefinitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WfdefinitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WfdefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
