interface IColumn {
  columnName: string;
  propertyName: string;
  isBoolean: boolean;
  sortDir: any;
}

export class Column implements IColumn {
  columnName: string;
  propertyName: string;
  isBoolean: boolean;
  sortDir: any;
}
