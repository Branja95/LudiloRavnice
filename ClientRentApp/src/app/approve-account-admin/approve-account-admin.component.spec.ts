import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApproveAccountAdminComponent } from './approve-account-admin.component';

describe('ApproveAccountAdminComponent', () => {
  let component: ApproveAccountAdminComponent;
  let fixture: ComponentFixture<ApproveAccountAdminComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApproveAccountAdminComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApproveAccountAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
