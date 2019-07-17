import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TestDataComponent } from './test-data.component';

describe('TestDataComponent', () => {
  let component: TestDataComponent;
  let fixture: ComponentFixture<TestDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TestDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TestDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
