import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EntityManageComponent } from './entity-manage.component';

describe('EntityManageComponent', () => {
  let component: EntityManageComponent;
  let fixture: ComponentFixture<EntityManageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EntityManageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EntityManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
