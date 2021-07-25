import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenderTemplateComponent } from './tender-template.component';

describe('TenderTemplateComponent', () => {
  let component: TenderTemplateComponent;
  let fixture: ComponentFixture<TenderTemplateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenderTemplateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenderTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
