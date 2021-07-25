export interface ITenderSection {
  Id: number;
  Section: string;
  ParentSection: string;
  Text: string;
  ConditionId: boolean;
}

export class TenderSection implements ITenderSection {
  constructor(Id, Section, ParentSection, Text, ConditionId) {
    this.Id = Id;
    this.Section = Section;
    this.ParentSection = ParentSection;
    this.Text = Text;
    this.ConditionId = ConditionId;
  }
  Id: number;
  Section: string;
  ParentSection: string;
  Text: string;
  ConditionId: boolean;

}

