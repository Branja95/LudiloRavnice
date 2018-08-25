import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApproveServiceAdminComponent } from './approve-service-admin.component';

describe('ApproveServiceAdminComponent', () => {
  let component: ApproveServiceAdminComponent;
  let fixture: ComponentFixture<ApproveServiceAdminComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApproveServiceAdminComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApproveServiceAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
