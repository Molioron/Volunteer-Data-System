import React, { useState, useEffect } from "react";
import { MultiSelect } from 'react-multi-select-component';
import './HomePage.css';

const HomeInputFields = ({ initialDate, lastDate, dateFilterType, onChange, area, jobTitle }) => {

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

  // Initialize selectedArea based on the area prop
  const [selectedArea, setSelectedArea] = useState(area.map(a => ({ label: a, value: a })));
  const [selectedJob, setSelectedJob] = useState(jobTitle.map(j => ({ label: j, value: j })));

  useEffect(() => {
    // Extract the values only and update the parent component's state
    onChange({ target: { name: 'volunteerAreas', value: selectedArea.map(area => area.value) } });
  }, [selectedArea]);

  useEffect(() => {
    // Extract the values only and update the parent component's state
    onChange({ target: { name: 'jobTypes', value: selectedJob.map(job => job.value) } });
  }, [selectedJob]);

  return (
    <div className="home-input-fields">
      <div className="form-group">
        <label>Area:</label>
        <MultiSelect 
          options={areas} 
          value={selectedArea} 
          onChange={setSelectedArea} 
          labelledBy="Select Area"
        />
      </div>

      <div className="form-group">
        <label>Job Title:</label>
        <MultiSelect 
          options={jobs} 
          value={selectedJob} 
          onChange={setSelectedJob} 
          labelledBy="Select Job Title"
        />
      </div>

      <div className="form-group">
        <label>Initial Date:</label>
        <input
          type="date"
          name="initialDate"
          value={initialDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>End Date:</label>
        <input
          type="date"
          name="lastDate"
          value={lastDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>Date Type:</label>
        <select name="dateFilterType" value={dateFilterType} onChange={(e) => onChange(e)}>
          <option value="">Select Date Type</option>
          <option value="Contains">Contains</option>
          <option value="Intersects">Intersects</option>
        </select>
      </div>
    </div>
  );
};

export default HomeInputFields;
