import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkflowSelectedComponent } from './workflow-selected.component';

describe('WorkflowSelectedComponent', () => {
  let component: WorkflowSelectedComponent;
  let fixture: ComponentFixture<WorkflowSelectedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkflowSelectedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkflowSelectedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
