import { TrendMarkerResult, TrendDataPoint, CoreDataHelper, CoreDataSet, Unit } from '../typings/FT';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';

export class DisplayOption {
    public static readonly AllIndicators = 0;
    public static readonly SelectedIndicator = 1;
}

export class SortType {
    public static readonly AtoZ = 0;
    public static readonly Value = 1;
    public static readonly Rank = 2;
}

export class ViewModes {
    public static readonly area = 0;
    public static readonly multiArea = 1;
}

export class Data {
    areaName: string;
    areaCode: string;
    indicatorName: string;
    period: string;
    newDataBadge: string;
    isNewData: boolean;
    unit: string;
    trendRows: TrendRow[];
    trendData: TrendData;
    trendSource: string;
    indicatorId: string;
}

export class TrendRow {
    period: string;
    trendMarkerResult: TrendMarkerResult;
    recentTrendImage: string;
    trendImage: string;
    count: string;
    value: string;
    lowerCI: string;
    upperCI: string;
    subNational: string;
    national: string;
    significance: number;
    highlightEngland: boolean;
    highlightSubnational: boolean;
    hoverCI: boolean;
    highlightAreaValue: boolean;
    useRagColours: boolean;
    useQuintileColouring: boolean;
    comparatorName: string;
    subNationalNoteId: number;
    nationalNoteId: number;
    subNationalSignificance: number;
    nationalSignificance: number;
    tableId: number;
    useTarget: boolean;
    targetLegendHtml: string;
    valueNoteId: number;
    polarityId: number;
    nationalData: CoreDataSet;
    subnationalData: CoreDataSet;
    areaData: TrendDataPoint;
    isLastRow: boolean;
}

export class TrendData {
    ftHelper: FTHelperService;
    benchmarkPoints: any[] = [];
    areaPoints: any[] = [];
    ciPoints: any[] = [];
    labels: string[] = [];
    unitLabel: string;
    radius: number;
    showConfidenceBars: boolean;
    max: number;
    min: number;
    areaName: string;
    indicatorName: string;
    width: number;
    height: number;
    areaCode: string;
    viewMode: number;
    trendDataPointsCount: number;
    comparatorName: string;
    unit: Unit;
    displayXAxisLabel: boolean;
    displayYAxisLabel: boolean;
    displayLegend: boolean;
    chartId: number;

    constructor(ftHelperService: FTHelperService) {
        this.ftHelper = ftHelperService;
    }

    addAreaPoint(trendDataPoint: TrendDataPoint, markerColour: string) {
        const info = this.ftHelper.newTrendDataInfo(trendDataPoint);

        this.areaPoints.push({
            y: info.isValue() ? trendDataPoint.D : null,
            data: trendDataPoint,
            color: markerColour,
            noteId: trendDataPoint.NoteId,
            marker: {
                fillColor: markerColour,
                states: {
                    hover: {
                        fillColor: markerColour
                    }
                }
            }
        });
    }

    addCIPoint(trendDataPoint: TrendDataPoint) {
        this.ciPoints.push([Number(trendDataPoint.LF), Number(trendDataPoint.UF)]);
    }

    addBenchmarkPoint(coreDataSet: CoreDataSet) {
        const info = this.ftHelper.newCoreDataSetInfo(coreDataSet);

        this.benchmarkPoints.push({
            y: info.isValue() ? coreDataSet.Val : null,
            data: coreDataSet,
            isBenchmark: true
        })
    }

    addLabel(label: string) {
        this.labels.push(label);
    }

    setUnitLabel(unitLabel: string) {
        this.unitLabel = unitLabel;
    }

    setRadius(trendDataPointsLength: number) {
        if (trendDataPointsLength > 40) {
            this.radius = 2;
        } else {
            this.radius = 5;
        }
    }

    setShowConfidenceBars(state: boolean) {
        this.showConfidenceBars = state;
    }
}

export class BenchmarkPoint {
    y: number;
    data: CoreDataSet;
    isBenchmark: boolean;
}
