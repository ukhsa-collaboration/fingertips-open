export class IndicatorMenuItem {
    IndicatorId: number
    IndicatorName: string;
    AgeId: number;
    SexId: number;
    Key: string;
    Sequence: number;
}

export class Point {
    Name: string;
    AreaCode: string;
    X: number;
    Y: number;
    XValF: string;
    YValF: string;
    Highlighted: boolean;
    Colour: string;
    Symbol: string;
}

export class IndicatorUnit {
    UnitX: string;
    UnitY: string;
}

export class DataSeries {
    Name: string;
    Points: Point[];
    XAxisTitle: string;
    YAxisTitle: string;
    IndicatorUnit: IndicatorUnit;
    Margin: number;
    XAxisData: number[];
    YAxisData: number[];
    R2Selected: boolean;
    SelectedAreaName: string;
}

export class LinearRegression {
    Coordinates: number[];
    Slope: number;
    Intercept: number;
    R2: number;
  }

  export class ChartSeries {
    Name: string;
    Type: string;
    Data: number[][];
    Coordinates: number[];
    Colour: string;
    MarkerSymbol: string;
    ShowNameInLegend: boolean;
  }
