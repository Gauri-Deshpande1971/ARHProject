import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContenttypesComponent } from './contenttypes.component';

describe('ContenttypesComponent', () => {
  let component: ContenttypesComponent;
  let fixture: ComponentFixture<ContenttypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ContenttypesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContenttypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
