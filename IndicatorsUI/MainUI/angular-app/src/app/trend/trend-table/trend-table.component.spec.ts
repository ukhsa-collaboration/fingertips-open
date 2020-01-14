import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { TrendTableComponent, TrendTableMarkerTooltopProvider } from './trend-table.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { TrendRow } from '../trend';
import { PolarityIds } from '../../shared/constants';
import { Significance } from '../../typings/FT';
import { ExportCsvComponent } from '../../shared/component/export-csv/export-csv.component';

describe('TrendTableComponent', () => {
  let component: TrendTableComponent;
  let fixture: ComponentFixture<TrendTableComponent>;

  let ftHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        TrendTableComponent,
        ExportCsvComponent
      ],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrendTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

describe('TrendTableMarkerTooltopProvider', () => {

  const row = new TrendRow();

  const checkText = function (text) {
    expect(new TrendTableMarkerTooltopProvider().getTooltipText(row)).toBe(text);
  }

  beforeEach(() => {
    // Set default values
    row.polarityId = PolarityIds.NotApplicable;
    row.comparatorName = 'E';
    row.useQuintileColouring = false;
    row.significance = Significance.None;
  });

  it('should create', () => {
    expect(new TrendTableMarkerTooltopProvider()).toBeTruthy();
  });

  it('test no significance', () => {
    checkText('Significance is not calculated for this indicator');
  });

  it('test lowest quintile', () => {
    row.useQuintileColouring = true;
    row.significance = 1;
    checkText('Lowest quintile in E');
  });

  it('test highest quintile', () => {
    row.useQuintileColouring = true;
    row.significance = 5;
    checkText('Highest quintile in E');
  });

  it('test best quintile', () => {
    row.useQuintileColouring = true;
    row.significance = 1;
    row.polarityId = PolarityIds.RAGLowIsGood;
    checkText('Best quintile in E');
  });

  it('test best quintile', () => {
    row.useQuintileColouring = true;
    row.significance = 5;
    row.polarityId = PolarityIds.RAGLowIsGood;
    checkText('Worst quintile in E');
  });

  it('test RAG worse', () => {
    row.significance = Significance.Worse;
    row.polarityId = PolarityIds.RAGLowIsGood;
    checkText('Significantly worse than E average');
  });

  it('test RAG similar', () => {
    row.significance = Significance.Same;
    row.polarityId = PolarityIds.RAGLowIsGood;
    checkText('Not significantly different to E average');
  });

  it('test RAG better', () => {
    row.significance = Significance.Better;
    row.polarityId = PolarityIds.RAGLowIsGood;
    checkText('Significantly better than E average');
  });

});

