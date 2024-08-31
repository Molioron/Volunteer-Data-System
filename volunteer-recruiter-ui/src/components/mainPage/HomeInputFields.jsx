import React, { useState, useEffect } from 'react';
import InputField from '../common/InputField'; 
import { MultiSelect } from 'react-multi-select-component'; 
import './HomePage.css';

const areas = [ 
  { label: "North", value: "North" },
  { label: "South", value: "South" },
  { label: "Central", value: "Central" },
];

const jobs = [
  { label: "Agriculture", value: "Agriculture" },
  { label: "Cooking", value: "Cooking" },
  { label: "Transportation", value: "Transportation" },
  { label: "AnimalCare", value: "AnimalCare" }
];

const HomeInputFields = ({ initialDate, lastDate, dateFilterType, onChange, area, jobTitle }) => {

  // State to manage selected areas and job types, initialized with the props provided
  const [selectedAreas, setSelectedAreas] = useState(area.map(a => ({ label: a, value: a })));
  const [selectedJobs, setSelectedJobs] = useState(jobTitle.map(j => ({ label: j, value: j })));

  // Effect hook to handle changes in selected areas
  useEffect(() => {
    const newVolunteerAreas = selectedAreas.map(area => area.value);
    if (newVolunteerAreas.join(',') !== area.join(',')) {
      onChange({ target: { name: 'volunteerAreas', value: newVolunteerAreas } });
    }
  }, [selectedAreas, area, onChange]); 

  // Effect hook to handle changes in selected jobs
  useEffect(() => {
    const newJobTypes = selectedJobs.map(job => job.value);
    if (newJobTypes.join(',') !== jobTitle.join(',')) {
      onChange({ target: { name: 'jobTypes', value: newJobTypes } });
    }
  }, [selectedJobs, jobTitle, onChange]);

  return (
    <div className="home-input-fields">
      <div className="form-group">
        <label>Area:</label>
        <MultiSelect 
          options={areas} 
          value={selectedAreas} 
          onChange={setSelectedAreas} 
          labelledBy="Select Area"
        />
      </div>

      <div className="form-group">
        <label>Job Title:</label>
        <MultiSelect 
          options={jobs} 
          value={selectedJobs} 
          onChange={setSelectedJobs} 
          labelledBy="Select Job Title"
        />
      </div>

      <InputField label="Initial Date:" type="date" name="initialDate" value={initialDate} onChange={onChange} />
      <InputField label="End Date:" type="date" name="lastDate" value={lastDate} onChange={onChange} />
      <InputField label="Date Type:" type="select" name="dateFilterType" value={dateFilterType} onChange={onChange} options={[
        { label: "Select Date Type", value: "" },
        { label: "Contains", value: "Contains" },
        { label: "Intersects", value: "Intersects" }
      ]} />
    </div>
  );
};

export default HomeInputFields;